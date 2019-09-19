using System.Collections;
using System.Collections.Generic;
using Components;
using Components.Interfaces;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AnimatorBlendStateSwitching = Components.Behaviours.AnimatorBlendStateSwitching;

namespace Tests
{
    public class animator_blend_state_switching
    {
        // A Test behaves as an ordinary method
        [Test]
        public void returns_illuminated_left_facing_animator_enum()
        {
            //arrange
            IAnimatorBlendSwitching component = Substitute.For<IAnimatorBlendSwitching>();
            AnimatorBlendStateSwitching behaviour = new AnimatorBlendStateSwitching(component);

            component.lightsOn.Returns(true);
            component.currentFacingDirection.Returns(new Vector2(0, 0));

            //act
            var calculatedEnum = behaviour.CalculatingAnimatorBlendState();
            
            //assert
            Assert.AreEqual((float)Enums.AnimatorBlendStates.IlluminatedFacingLeft, calculatedEnum);
        }
        
        [Test]
        public void returns_illuminated_right_facing_animator_enum()
        {
            //arrange
            IAnimatorBlendSwitching component = Substitute.For<IAnimatorBlendSwitching>();
            AnimatorBlendStateSwitching behaviour = new AnimatorBlendStateSwitching(component);

            component.lightsOn.Returns(true);
            component.currentFacingDirection.Returns(new Vector2(1, 0));

            //act
            var calculatedEnum = behaviour.CalculatingAnimatorBlendState();
            
            //assert
            Assert.AreEqual((float)Enums.AnimatorBlendStates.IlluminatedFacingRight, calculatedEnum);
        }
        
        [Test]
        public void returns_darkened_left_facing_animator_enum()
        {
            //arrange
            IAnimatorBlendSwitching component = Substitute.For<IAnimatorBlendSwitching>();
            AnimatorBlendStateSwitching behaviour = new AnimatorBlendStateSwitching(component);

            component.lightsOn.Returns(false);
            component.currentFacingDirection.Returns(new Vector2(0, 0));

            //act
            var calculatedEnum = behaviour.CalculatingAnimatorBlendState();
            
            //assert
            Assert.AreEqual((float)Enums.AnimatorBlendStates.DarkenedFacingLeft, calculatedEnum);
        }
        
        [Test]
        public void returns_darkened_right_facing_animator_enum()
        {
            //arrange
            IAnimatorBlendSwitching component = Substitute.For<IAnimatorBlendSwitching>();
            AnimatorBlendStateSwitching behaviour = new AnimatorBlendStateSwitching(component);

            component.lightsOn.Returns(false);
            component.currentFacingDirection.Returns(new Vector2(1, 0));

            //act
            var calculatedEnum = behaviour.CalculatingAnimatorBlendState();
            
            //assert
            Assert.AreEqual((float)Enums.AnimatorBlendStates.DarkenedFacingRight, calculatedEnum);
        }        
    }
}
