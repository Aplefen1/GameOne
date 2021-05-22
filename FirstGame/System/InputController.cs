using System;
using System.Text;
using FirstGame.Entities;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace FirstGame.System
{

    public class InputController
    {
        private Trex _trex;
        private KeyboardState _previousKeyboardState;
        private bool _isBlocked;

        public InputController(Trex trex)
        {
            _trex = trex;
        }

        public void ProcessControls(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if(!_isBlocked)
            {

                bool isJumpPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Space);
                bool wasJumpPressed = _previousKeyboardState.IsKeyDown(Keys.Up) || _previousKeyboardState.IsKeyDown(Keys.Space);

                if(!wasJumpPressed && isJumpPressed)
                {

                    if(_trex.State != TrexState.Jumping)
                        _trex.BeginJump();

                }
                else if(_trex.State == TrexState.Jumping && !isJumpPressed)
                {
                    _trex.CancelJump();
                }

                else if(keyboardState.IsKeyDown(Keys.Down))
                {

                    if (_trex.State == TrexState.Jumping || _trex.State == TrexState.Falling)
                        _trex.Drop();
                    else
                        _trex.Duck();

                }
                else if(!keyboardState.IsKeyDown(Keys.Down) && _trex.State == TrexState.Ducking)
                {

                    _trex.GetUp();

                }
                
            }
            _isBlocked = false;
            _previousKeyboardState = keyboardState;

        }

        public void BlockInputTemporarily()
        {
            _isBlocked = true;
        }

    }
}