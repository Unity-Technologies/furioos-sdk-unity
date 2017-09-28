using UnityEngine;
using UnityEngine.UI;

namespace Rise.App.ViewModels {
    public class LoadingViewModel : MonoBehaviour {
        public Image background;
        public Image progress;
        public Image backdrop;

        public void Opaque() {
            Color bgColor = background.color;
            Color pColor = progress.color;
            Color bColor = backdrop.color;

            bgColor.a = 1.0f;
            pColor.a = 1.0f;
            bColor.a = 1.0f;

            background.color = bgColor;
            progress.color = pColor;
            backdrop.color = bColor;
        }

        public void AutoFill() {
            LeanTween.value(gameObject, 0.0f, 1.0f, 0.8f).setOnUpdate((float value) => {
                progress.fillAmount = value;
            }).setLoopPingPong();
        }

        public void Destroy() {
            Destroy(gameObject);
        }

        public void FadeOut() {
            Color bgColor = background.color;
            Color pColor = progress.color;
            Color bColor = backdrop.color;

            LeanTween.value(gameObject, 1.0f, 0.0f, 0.4f).setOnUpdate((float value) => {
                bgColor.a = value;
                pColor.a = value;
                bColor.a = value;

                background.color = bgColor;
                progress.color = pColor;
                backdrop.color = bColor;
            }).setOnComplete(() => {
                Destroy();
            });
        }
    }
}