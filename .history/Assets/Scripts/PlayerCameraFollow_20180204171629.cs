﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour {

	public Controller2D target;
	public Vector2 focusAreaSize;

	public float verticalOffset;
	public float lookAheadDistanceX;
	public float lookAheadDistanceY;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;

	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;

	float currentLookAheadY;
	float targetLookAheadY;
	float lookAheadDirY;
	float smoothVelocityY;

	bool lookAheadStopped;

	void Start() {
		focusArea = new FocusArea (target.collider.bounds, focusAreaSize);
	}

	//Update after all player movement has finished
	void LateUpdate() {
		focusArea.Update (target.collider.bounds);

		Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

		if (focusArea.velocity.x != 0) {
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0) {
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDistanceX;
			}
			else {
				if (!lookAheadStopped) {
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistanceX - currentLookAheadX) / 4f;
				}
			}
		}

		// if (focusArea.velocity.y != 0) {
		// 	lookAheadDirY = Mathf.Sign(focusArea.velocity.y);
		// 	if (Mathf.Sign(target.playerInput.y) == Mathf.Sign(focusArea.velocity.y) && target.playerInput.y != 0) {
		// 		lookAheadStopped = false;
		// 		targetLookAheadY = (lookAheadDirY * lookAheadDistanceY) + (-1 * lookAheadDirY * verticalOffset);
		// 	}
		// 	else {
		// 		if (!lookAheadStopped) {
		// 			lookAheadStopped = true;
		// 			targetLookAheadY = currentLookAheadY + (lookAheadDirY * lookAheadDistanceY - currentLookAheadY) / 4f;
		// 		}
		// 	}
		// }

		
		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
		currentLookAheadY = Mathf.SmoothDamp(currentLookAheadY, targetLookAheadY, ref smoothVelocityY, verticalSmoothTime);
		
		//focusPosition.y += Vector2.up * currentLookAheadY; //Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += (Vector2.right * currentLookAheadX); // + (Vector2.up * currentLookAheadY); // (Mathf.Abs(currentLookAheadY) > verticalOffset ? currentLookAheadY : verticalOffset));

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color(1,0,0,0.5f);
		Gizmos.DrawCube(focusArea.center,focusAreaSize);
	}

	struct FocusArea {
		public Vector2 center;
		public Vector2 velocity;
		float left, right;
		float top, bottom;

		public FocusArea (Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			center = new Vector2((left+right)/2, (top+bottom)/2);
		}

		public void Update(Bounds targetBounds) {
			
			//target is pushing against left or right sides
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;
			
			//target is pushing against top or bottom
			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			bottom += shiftY;
			top += shiftY;

			//update center
			center = new Vector2((left+right)/2, (top+bottom)/2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}
}
