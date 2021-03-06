﻿namespace AceJump
{
    using System.Linq;
    using System.Windows.Input;
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IKeyProcessorProvider))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Name("AceKeyProcessor")]
    internal sealed class AceKeyProcessorProvider : IKeyProcessorProvider
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("AceJump")]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        public AdornmentLayerDefinition editorAdornmentLayer = null;


        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            var aceJump = new AceJump();
            var aceKeyProcessor = new AceKeyProcessor(aceJump);

            aceJump.SetView(wpfTextView);
            return aceKeyProcessor;
        }
    }

    public class AceKeyProcessor : KeyProcessor
    {

        private JumpControler jumpControler;

        public AceKeyProcessor(AceJump aceJump)
        {
            this.jumpControler = new JumpControler(aceJump);
        }

        public override void PreviewKeyUp(KeyEventArgs args)
        {
            if (this.jumpControler.Active())
            {
                if (args.Key == Key.Escape || args.Key == Key.Left || args.Key == Key.Right || args.Key == Key.Up || args.Key == Key.Down)
                {
                    this.jumpControler.Close();
                }
            }
            base.KeyDown(args);
        }

        public override void KeyDown(KeyEventArgs args)
        {
            if (Keyboard.IsKeyDown(Key.OemSemicolon) && Keyboard.IsKeyDown(Key.LeftCtrl)
                && Keyboard.IsKeyDown(Key.LeftAlt))
            {
                this.jumpControler.ShowJumpEditor();
                args.Handled = true;
                return;
            }

            string key = KeyUtility.GetKey(args.Key);
            if (!string.IsNullOrEmpty(key))
            {
                bool handled = this.jumpControler.ControlJump(key.FirstOrDefault());

                args.Handled = handled;
            }
        }
    }
}
    