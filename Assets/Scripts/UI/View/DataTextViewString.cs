namespace WMK
{
    public class DataTextViewString : DataTextView<string>
    {
        protected override void UpdateView(string data)
        {
            m_text.text = data.ToString();
        }
    }
}
