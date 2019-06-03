// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*++                                                                         
    Abstract:
        This file contains the definition of a class that defines
        the common functionality required to serialize a PageContent.                                                                     
--*/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Printing;
using System.Security;
using System.Security.Permissions;
using System.ComponentModel.Design.Serialization;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;

namespace System.Windows.Xps.Serialization
{
    /// <summary>
    /// Class defining common functionality required to
    /// serialize a ReachPageContentSerializer.
    /// </summary>
    internal class ReachPageContentSerializer :
                   ReachSerializer
    {
        #region Constructor

        /// <summary>
        /// Constructor for class ReachPageContentSerializer
        /// </summary>
        /// <param name="manager">
        /// The serialization manager, the services of which are
        /// used later in the serialization process of the type.
        /// </param>
        public
        ReachPageContentSerializer(
            PackageSerializationManager   manager
            ):
        base(manager)
        {
            
        }

        #endregion Constructor
        
        #region Internal Methods

        /// <summary>
        /// The method is called once the object data is discovered at that 
        /// point of the serialization process.
        /// </summary>
        /// <param name="serializableObjectContext">
        /// The context of the object to be serialized at this time.
        /// </param>
        internal
        override
        void
        PersistObjectData(
            SerializableObjectContext   serializableObjectContext
            )
        {
            if(serializableObjectContext.IsComplexValue)
            {
                //SerializeObjectCore(serializableObjectContext);

                FixedPage fixedPage = Toolbox.GetPageRoot(serializableObjectContext.TargetObject);

                if(fixedPage != null)
                {
                    ReachSerializer serializer = SerializationManager.GetSerializer(fixedPage);

                    if(serializer!=null)
                    {
                        //Get FixedPage Print Ticket and cache it inside of the SerializationManager
                        //so we can get it inside of 
                        XpsSerializationPrintTicketRequiredEventArgs e =
                            new XpsSerializationPrintTicketRequiredEventArgs(PrintTicketLevel.FixedPagePrintTicket,
                                                             0);
                        ((IXpsSerializationManager)SerializationManager).OnXPSSerializationPrintTicketRequired(e);

                        PrintTicket printTicket = null;
                        if( e.Modified )
                        {
                            printTicket =  e.PrintTicket;
                        }
                        Toolbox.Layout(fixedPage, printTicket);
                        
                        ((IXpsSerializationManager)SerializationManager).FixedPagePrintTicket = printTicket;

                        serializer.SerializeObject(fixedPage);
                    }
                    else
                    {
                        throw new XpsSerializationException(SR.Get(SRID.ReachSerialization_NoSerializer));
                    }
                }
            }
            else
            {
                throw new XpsSerializationException(SR.Get(SRID.ReachSerialization_WrongPropertyTypeForPageContent));
            }
        }

        #endregion Internal Methods
    };
}
