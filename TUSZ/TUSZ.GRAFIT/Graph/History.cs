﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;

namespace TUSZ.GRAFIT.Graph
{
    public struct History
    {
        public HashSet<Route> usedRoutes;
        public Route lastUsedRoute;
        public List<Instruction> instructions;
        public Instruction lastInstruction;
        public double totalWalkingTime;
        public int totalDistance;
        public int lastStopTimeIndex;

        public History Clone()
        {
            return new History
            {
                instructions = this.instructions,
                lastUsedRoute = this.lastUsedRoute,
                totalDistance = this.totalDistance,
                totalWalkingTime = this.totalWalkingTime,
                usedRoutes = this.usedRoutes,
                lastInstruction = this.lastInstruction,
                lastStopTimeIndex = this.lastStopTimeIndex
            };
        }

        public static History CreateEmptyHistory()
        {
            return new History
                {
                    instructions = new List<Instruction>(),
                    lastUsedRoute = null,
                    usedRoutes = new HashSet<Route>(),
                    totalWalkingTime = 0,
                    totalDistance = 0,
                    lastInstruction = null,
                    lastStopTimeIndex = -1
                };
        }

        public History AppendInstruction(Instruction instruction, int addDistance)
        {
            var result = this.Clone();
            result.instructions = new List<Instruction>(this.instructions);
            result.instructions.Add(instruction);
            result.lastInstruction = instruction;
            result.usedRoutes = new HashSet<Route>(this.usedRoutes);
            result.usedRoutes.Add(instruction.route);
            result.lastUsedRoute = instruction.route;
            result.totalDistance = this.totalDistance + addDistance;
            return result;
        }
    }
}
