////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2011 - ADN/Developer Technical Services
//
// This software is provided as is, without any warranty that it will work. You choose to use this tool at your own risk.
// Neither Autodesk nor the author Philippe Leefsma can be taken as responsible for any damage this tool can cause to 
// your data. Please always make a back up of your data prior to use this tool, as it will modify the documents involved 
// in the feature transformation.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Inventor;

namespace ThreadModeler
{
    /////////////////////////////////////////////////////////////////
    // Provides basic Inventor utilities
    //
    /////////////////////////////////////////////////////////////////
    class Toolkit
    {
        /////////////////////////////////////////////////////////////
        // Members
        //
        /////////////////////////////////////////////////////////////
        private static double _Tolerance = 0.0001;

        private static Inventor.Application _Application;
        private static TransientGeometry _Tg;

        private static bool _ConstructionWorkFeature = true;

        /////////////////////////////////////////////////////////////
        // Use: Initializes the Toolkit library
        //
        /////////////////////////////////////////////////////////////
        public static void Initialize(
            Inventor.Application Application)
        {
            _Application = Application;

            _Tg = _Application.TransientGeometry;
        }

        /////////////////////////////////////////////////////////////
        // Use: Returns Normal as UnitVector for input Face.
        //
        /////////////////////////////////////////////////////////////
        public static UnitVector GetFaceNormal(Face face)
        {
            SurfaceEvaluator oEvaluator = face.Evaluator;

            double[] point = new double[3] 
            { 
              face.PointOnFace.X, 
              face.PointOnFace.Y, 
              face.PointOnFace.Z 
            };

            double[] guessParams = new double[2] {0.0, 0.0};
            double[] maxDev = new double[2] {0.0, 0.0};
            double[] Params = new double[2] {0.0, 0.0};

            SolutionNatureEnum[] sol = new SolutionNatureEnum[1] 
            { 
                SolutionNatureEnum.kUnknownSolutionNature 
            };

            oEvaluator.GetParamAtPoint(ref point, 
                ref guessParams, 
                ref maxDev, 
                ref Params, 
                ref sol);

            double[] normal = new double[3];

            oEvaluator.GetNormal(ref Params, ref normal);

            return _Tg.CreateUnitVector(
                normal[0], 
                normal[1], 
                normal[2]);
        }

        /////////////////////////////////////////////////////////////
        // Use: Returns an UnitVector orthogonal to input vector
        //
        /////////////////////////////////////////////////////////////
        public static UnitVector GetOrthoVector(UnitVector vector)
        {
            if (Math.Abs(vector.Z) < _Tolerance)
            {
                return _Application.TransientGeometry.
                    CreateUnitVector(0, 0, 1);
            }
            else if (Math.Abs(vector.Y) < _Tolerance)
            {
                return _Application.TransientGeometry.
                    CreateUnitVector(0, 1, 0);
            }
            else
            {
                //Expr: - xx'/y = y'
                return _Application.TransientGeometry.
                    CreateUnitVector(1, -vector.X / vector.Y, 0);
            }
        }

        /////////////////////////////////////////////////////////////
        // Use: Returns two orthogonal vectors 
        //      depending on the input normal
        //
        /////////////////////////////////////////////////////////////
        public static void GetOrthoBase(UnitVector normal,
            out UnitVector xAxis,
            out UnitVector yAxis)
        {
            xAxis = GetOrthoVector(normal);

            yAxis = normal.CrossProduct(xAxis);
        }

        /////////////////////////////////////////////////////////////
        // Use: Inserts new sketch in part and copies content
        //      from input sketch
        /////////////////////////////////////////////////////////////
        public static PlanarSketch InsertSketch(PartDocument doc, 
            PlanarSketch sketch, 
            UnitVector xAxis, 
            UnitVector yAxis,
            Point basePoint)
        {
            PartComponentDefinition compDef = 
                doc.ComponentDefinition;

            WorkAxis wa1 = compDef.WorkAxes.AddFixed(
                basePoint, xAxis, _ConstructionWorkFeature);

            WorkAxis wa2 = compDef.WorkAxes.AddFixed(
                basePoint, yAxis, _ConstructionWorkFeature);

            WorkPlane wp = compDef.WorkPlanes.AddByTwoLines(
                wa1, wa2, _ConstructionWorkFeature);

            WorkPoint origin = compDef.WorkPoints.AddFixed(
                basePoint, _ConstructionWorkFeature);

            PlanarSketch newSketch = 
                compDef.Sketches.AddWithOrientation(
                    wp, wa1, true, true, origin, false);
            
            sketch.CopyContentsTo(newSketch as Sketch);
            
            return newSketch;
        }

        /////////////////////////////////////////////////////////////
        // Use: Finds a parameter based on comment text
        //
        /////////////////////////////////////////////////////////////
        public static bool FindParameter(PartDocument doc, 
            string comment)
        {
            foreach (Parameter parameter in 
                doc.ComponentDefinition.Parameters)
            {
                if (parameter.Comment == comment)
                {
                    return true;
                }
            }

            return false;
        }

        /////////////////////////////////////////////////////////////
        // Use: Finds a parameter and update name and comment
        //      based on sketch name
        /////////////////////////////////////////////////////////////
        public static Parameter FindAndUpdateParameter(
            PartDocument doc, 
            string comment, 
            string sketchName)
        {
            foreach(Parameter parameter in 
                doc.ComponentDefinition.Parameters)
            {
                if(parameter.Comment == comment)
                {
                    parameter.Comment = sketchName + " " + comment;
                    parameter.Name = sketchName + "_" + comment;

                    return parameter;
                }
            }

            return null;
        }

        /////////////////////////////////////////////////////////////
        // Use: Late-binded method to retrieve ObjectType property
        //
        /////////////////////////////////////////////////////////////
        public static ObjectTypeEnum GetInventorType(Object obj)
        {
            try
            {
                System.Object objType = obj.GetType().InvokeMember(
                    "Type", 
                    System.Reflection.BindingFlags.GetProperty,
                    null,
                    obj,
                    null,
                    null, null, null);

                return (ObjectTypeEnum)objType;
            }
            catch
            {
                //error...
                return ObjectTypeEnum.kGenericObject;
            }
        }

        /////////////////////////////////////////////////////////////
        // Use: Late-binded method to get object property based
        //      on name.
        /////////////////////////////////////////////////////////////
        public static System.Object GetProperty(System.Object obj, 
            string property)
        {
            try
            {
                System.Object objType = obj.GetType().InvokeMember(
                    property,
                    System.Reflection.BindingFlags.GetProperty,
                    null,
                    obj,
                    null,
                    null, null, null);

                return objType;
            }
            catch
            {
                return null;
            }
        }

        /////////////////////////////////////////////////////////////
        // Use: Returns Normal as UnitVector for input Face.
        //
        /////////////////////////////////////////////////////////////
        public static UnitVector GetFaceNormal(
            Face face, 
            Point point)
        {
            SurfaceEvaluator oEvaluator = face.Evaluator;

            Double[] Points = { point.X, point.Y, point.Z };

            Double[] GuessParams = new Double[2];
            Double[] MaxDev = new Double[2];
            Double[] Params = new Double[2];
            SolutionNatureEnum[] sol = new SolutionNatureEnum[2];

            oEvaluator.GetParamAtPoint(
                ref Points, 
                ref GuessParams, 
                ref MaxDev, 
                ref Params, 
                ref sol);

            Double[] normal = new Double[3];

            oEvaluator.GetNormal(ref Params, ref normal);

            return _Tg.CreateUnitVector(
                normal[0], 
                normal[1], 
                normal[2]);
        }

        /////////////////////////////////////////////////////////////
        // Use: Determines if a Cylindrical Face is an interior
        //      face or not. This methods works only for cylindrical
        //      geometry.
        /////////////////////////////////////////////////////////////
        public static bool IsInteriorFace(Face face)
        {
            Point basePoint = Toolkit.GetProperty(face.Geometry, 
                "BasePoint") as Point;

            UnitVector normal = Toolkit.GetFaceNormal(face, 
                face.PointOnFace);

            UnitVector refvect = 
                face.PointOnFace.VectorTo(basePoint).AsUnitVector();

            return (normal.DotProduct(refvect) > 0);
        }

        /////////////////////////////////////////////////////////////
        // Use: Returns direction of the cone side as Line object.
        //
        /////////////////////////////////////////////////////////////
        public static Line GetConeSideDirection(
            Cone cone, 
            UnitVector xAxis)
        {
            UnitVector yAxis = cone.AxisVector;

            double height = 
                (cone.IsExpanding ? -1.0 : 1.0) 
                * cone.Radius / Math.Tan(cone.HalfAngle);

            Point p1 = _Tg.CreatePoint(
                cone.BasePoint.X + xAxis.X * cone.Radius,
                cone.BasePoint.Y + xAxis.Y * cone.Radius,
                cone.BasePoint.Z + xAxis.Z * cone.Radius);

            Point p2 = _Tg.CreatePoint(
               cone.BasePoint.X + yAxis.X * height,
               cone.BasePoint.Y + yAxis.Y * height,
               cone.BasePoint.Z + yAxis.Z * height);

            return _Tg.CreateLine(p1, p1.VectorTo(p2));
        }

        public static Line GetFaceSideDirection(Face face, UnitVector xAxis)
        {
            switch (face.SurfaceType)
            {
                case SurfaceTypeEnum.kConeSurface:
                    Cone cone = face.Geometry as Cone;
                    UnitVector axisVector = cone.AxisVector;
                    double num = (cone.IsExpanding ? -1.0 : 1.0) * cone.Radius / Math.Tan(cone.HalfAngle);
                    Point point1 = Toolkit._Tg.CreatePoint(cone.BasePoint.X + xAxis.X * cone.Radius, cone.BasePoint.Y + xAxis.Y * cone.Radius, cone.BasePoint.Z + xAxis.Z * cone.Radius);
                    Point point2 = Toolkit._Tg.CreatePoint(cone.BasePoint.X + axisVector.X * num, cone.BasePoint.Y + axisVector.Y * num, cone.BasePoint.Z + axisVector.Z * num);
                    return Toolkit._Tg.CreateLine(point1, point1.VectorTo(point2));
                default:
                    return (Line)null;
            }
        }


        /////////////////////////////////////////////////////////////
        // Use: Determines if input Face has iMate attached to it
        //      and returns iMate name if any.
        /////////////////////////////////////////////////////////////
        public static bool HasiMate(
            Face face, 
            out string iMateName)
        {
            iMateName = string.Empty;

            PartComponentDefinition compDef =
                face.SurfaceBody.ComponentDefinition 
                    as PartComponentDefinition;

            foreach (iMateDefinition def in compDef.iMateDefinitions)
            { 
                object Entity = Toolkit.GetProperty(def, "Entity");

                if(Entity == null)
                    //Case CompositeiMateDefinition
                    continue;

                if (Entity == face)
                {
                    iMateName = def.Name;
                    return true;
                }

                if (Entity is Edge)
                {
                    foreach (Edge edge in face.Edges)
                    {
                        if (Entity == edge)
                        {
                            iMateName = def.Name;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}