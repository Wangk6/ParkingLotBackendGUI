﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu;
using System.Threading;
using Dapper;
using System.Windows.Threading;
using System.Data;
using System.Data.SqlClient;


namespace Carz
{
    public class VideoInterpreter
    {
        private bool showWindow;

        private int oldNumCarsInLot;
        private int numCarsInLot;
        private int steady;
        private int frCt;

        private string parkingLotName;
        private int parkingLotId;
        private string pathToVideoSource;

        private bool didStart;

        private Action<VideoInterpreter> CarDidEnter;
        private Action<VideoInterpreter> CarDidLeave;
        private Action<VideoInterpreter> CarProcessingDone;

        private double fps;
        private double frameWidth;
        private double frameHeight;

        private Emgu.CV.VideoCapture vCapture;
        private Emgu.CV.CascadeClassifier casc;
        private bool didStop;
        private bool didEnter;

        private Dispatcher dispatcher;

        public VideoInterpreter(string pathToVideoSource, string pathToXmlFile, Dispatcher dispatcher)
        {
            //sets variables that are not passed
            this.showWindow = false;
            numCarsInLot = 0;
            steady = 0;
            oldNumCarsInLot = 0;
            fps = 60;
            frameWidth = 1920;
            frameHeight = 1080;
            didStart = false;
            didStop = false;
            didEnter = false;
            frCt = 0;

            CarDidEnter = null;
            CarDidLeave = null;

            this.pathToVideoSource = pathToVideoSource;
            this.dispatcher = dispatcher;

            //sets up video stream pull
            vCapture = new Emgu.CV.VideoCapture(pathToVideoSource);
            if (fps > 0) vCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, fps);
            vCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, frameWidth);
            vCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, frameHeight);
            casc = new Emgu.CV.CascadeClassifier(pathToXmlFile);
        }

        public async void start()
        {
            if (didStart) return;

            if (this.CarDidEnter == null) CarDidEnter = CarDidEnterDefault;
            if (this.CarDidLeave == null) CarDidLeave = CarDidLeaveDefault;
            if (this.CarProcessingDone == null) CarProcessingDone = CarProcessingDoneDefault;

            //flag to note process started to prevent changing peramaters that would break the processing
            didStart = true;

            //sets up the image matrixs for the input raw and output processed
            Emgu.CV.Mat iMatrix = new Emgu.CV.Mat();
            Emgu.CV.Mat oMatrix = new Emgu.CV.Mat();

            await Task.Run(() =>
            {



                //creates a window if desired, window for testing purposes 
                if (showWindow) Emgu.CV.CvInvoke.NamedWindow("Car Detection Test", Emgu.CV.CvEnum.NamedWindowType.FreeRatio);


                //This async task continually pulls the video frame to be processed.
                Task.Run(() =>
                {
                    for (; ; )
                    {
                        try
                        {
                            if (didStop) return;
                            vCapture.Read(iMatrix);
                            if (iMatrix == null) { iMatrix.Dispose(); iMatrix = null; return; }
                            if (iMatrix.IsEmpty) { iMatrix.Dispose(); iMatrix = null; return; }
                            //System.Console.Out.WriteLine(iMatrix.Size.ToString());
                            // Emgu.CV.CvInvoke.WaitKey((int)(1000/(fps)));
                            //double FrameRate = vCapture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                            //System.Diagnostics.Debug.WriteLine(FrameRate);
                            if (fps > 0) Thread.Sleep((int)(1000.0 / fps));
                        }
                        catch (Exception e) { return; }
                    }
                });

                //This async task continually process the pulled frames
                for (; ; )
                {
                    try
                    {

                        //If the matrix used to store the frames is not empty
                        if (iMatrix == null) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                        if (!iMatrix.IsEmpty)
                        {
                            didEnter = true;

                            if (didStop) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                            //Converts the contents of the imatrix to greyscale, export results to omatrix
                            //this is to ensure proper processing
                            Emgu.CV.CvInvoke.CvtColor(iMatrix, oMatrix, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                            //Uses the cascade xml file provided in the initalizer to draw rectangles arround possible canditates.
                            Rectangle[] rects = casc.DetectMultiScale(oMatrix, 1.01, 5, new Size(700, 700), new Size(1100, 1100));

                            //removes the image from the out matrix if one exists to make room for the new one.
                            if (oMatrix == null) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                            if (oMatrix.IsEmpty) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }

                            for (int i = 0; i < oMatrix.Total.ToInt32(); i++) oMatrix.PopBack(1);


                            //sets inital value to zero
                            int carsInFrame = 0;

                            //loops through all of the rectangles in the discorvered object array
                            foreach (Rectangle rect in rects)
                            {
                                //draws the rectangles on the imatrix image for display if we wish to show the window
                                //we use imatrix as it is in color
                                if (showWindow)
                                {
                                    var x = rect.X;
                                    var y = rect.Y;
                                    var w = rect.Width;
                                    var h = rect.Height;
                                    Emgu.CV.CvInvoke.Rectangle(iMatrix, new Rectangle(x, y, w, h), new Emgu.CV.Structure.MCvScalar(50));

                                
                                }
                                //increase the number of cars in frame for each iteration
                                carsInFrame++;
                            }

                            if (carsInFrame == numCarsInLot) steady++;
                            else steady = 0;

                            //update the number of cars
                            numCarsInLot = carsInFrame;

                            //if the number of cars has changed
                            //call the proper delagte the necessary amount of times
                            if (carsInFrame > oldNumCarsInLot && steady > 20) { for (int i = 0; i < carsInFrame - oldNumCarsInLot; i++) dispatcher.Invoke(CarDidEnter, this); oldNumCarsInLot = numCarsInLot; }
                            if (carsInFrame < oldNumCarsInLot && steady > 20) { for (int i = 0; i < oldNumCarsInLot - carsInFrame; i++) dispatcher.Invoke(CarDidLeave, this); oldNumCarsInLot = numCarsInLot; }

                            //if the show window flag is true we push the drawn images to the window
                            if (showWindow)
                            {
                                if (iMatrix == null) { oMatrix.Dispose(); ; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                                if (iMatrix.IsEmpty) { oMatrix.Dispose(); ; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                                Emgu.CV.CvInvoke.Imshow("Car Detection Test", iMatrix);
                            }

                            //discard the now rendered frame
                            if (iMatrix == null) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                            if (iMatrix.IsEmpty) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                            iMatrix.PopBack(1);

                            //Distroys windows and stops loop if the escape key is pressed
                            if (showWindow && Emgu.CV.CvInvoke.WaitKey(33) == 27)
                            {
                                if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows();
                                break;
                            }
                            if (didStop) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                        }
                        else if (didEnter) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                    }
                    catch (Exception e) { oMatrix.Dispose(); oMatrix = null; dispatcher.Invoke(CarProcessingDone, this); if (showWindow) Emgu.CV.CvInvoke.DestroyAllWindows(); return; }
                }
            });
        }

        public void stop() { didStop = true; }

        public void setCarProcessingDone(Action<VideoInterpreter> func) { if (!didStart) CarProcessingDone = func; }

        //This will be called every time a car leaves async
        public void setCarDidLeaveDelegate(Action<VideoInterpreter> func) { if (!didStart) CarDidLeave = func; }

        //This will be called every time a car enteres async
        public void setCarDidEnterDelegate(Action<VideoInterpreter> func) { if (!didStart) CarDidEnter = func; }

        //set this flag for testing, will make a window to see the results. default false
        public void setShowWindow(bool flag) { if (!didStart) this.showWindow = flag; }

        //overides the 30fps count if needed
        public void setfps(double fps) { if (!didStart) if (fps > 0) { this.fps = fps; vCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, fps); } }

        //overides the 1920 frame width if needed
        public void setFrameWidth(double width) { if (!didStart) this.frameWidth = width; }

        //overides the 1080 frame height if needed
        public void setFrameHeight(double height) { if (!didStart) this.frameHeight = height; }

        //some getters that can be useful in delegate functions
        public string getPathToVideoSource() { return pathToVideoSource; }
        public int getParkingLotId() { return parkingLotId; }
        public string getParkingLotName() { return parkingLotName; }

        /*
         Examples of delegate usage...

         public void CarDidLeave(DateTime timeCarLeft, VideoInterpreter sender) 
         {
         timeCarLeft is the time this was called
         use the sender object to get attributes such as the lot id and lot name
         }

         public void CarDidEnter(DateTime timeCarEntered, VideoInterpreter sender) 
         {
         timeCarLeft is the time this was called
         use the sender object to get attributes such as the lot id and lot name
         } 
         
         */



        private void CarProcessingDoneDefault(VideoInterpreter sender) { }

        private void CarDidEnterDefault(VideoInterpreter sender)
        {
            /*
            using (SqlConnection connection = new SqlConnection(SQLConnection.ConnString("ParkingLotDB")))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("spCarParked", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ParkingLotID", sender.getParkingLotId()));
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        System.Console.Out.Write("Success");
                    }
                }
                catch (Exception e)
                {
                    System.Console.Out.Write(e.ToString());
                }
            }
            */
        }


        private void CarDidLeaveDefault(VideoInterpreter sender)
        {
            /*
            using (SqlConnection connection = new SqlConnection(SQLConnection.ConnString("ParkingLotDB")))
            {

                try
                {
                    using (SqlCommand command = new SqlCommand("spCarLeft", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ParkingLotID", sender.getParkingLotId()));
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        System.Console.Out.Write("Success");
                    }
                }
                catch (Exception e)
                {
                    System.Console.Out.Write(e.ToString());
                }
            }
            */
        }
    }
}