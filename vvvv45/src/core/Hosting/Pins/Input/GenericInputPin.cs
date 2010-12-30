﻿using System;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Hosting.Pins.Input
{
	public class GenericInputPin<T> : DiffPin<T>, IPinUpdater
	{
		protected INodeIn FNodeIn;
		
		public GenericInputPin(IPluginHost host, InputAttribute attribute)
			: base(host, attribute)
		{
			host.CreateNodeInput(attribute.Name, (TSliceMode)attribute.SliceMode, (TPinVisibility)attribute.Visibility, out FNodeIn);
			FNodeIn.SetSubType(new Guid[] { typeof(T).GUID }, typeof(T).FullName);
			
			base.InitializeInternalPin(FNodeIn);
		}
		
		protected override bool IsInternalPinChanged
		{
			get
			{
				return FNodeIn.PinIsChanged;
			}
		}
		
		unsafe protected override void DoUpdate()
		{
			SliceCount = FNodeIn.SliceCount;
			
			INodeIOBase usI;
			FNodeIn.GetUpstreamInterface(out usI);
			var upstreamInterface = usI as IGenericIO;
			
			for (int i = 0; i < FSliceCount; i++)
			{
				int usS;
				if (upstreamInterface != null)
				{
					FNodeIn.GetUpsreamSlice(i, out usS);
					FBuffer[i] = (T) upstreamInterface.GetSlice(usS);
				}
				else
				{
					FBuffer[i] = default(T);
				}
			}
		}
	}
}
