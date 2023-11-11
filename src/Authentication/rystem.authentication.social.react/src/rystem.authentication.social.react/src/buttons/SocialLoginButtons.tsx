import { getSocialLoginSettings, getAmazonButton, getGoogleButton, getMicrosoftButton, getFacebookButton, getGitHubButton, getLinkedinButton, SocialButtonValue } from "..";

const getButtons = new Array<() => SocialButtonValue>;
getButtons.push(getGoogleButton);
getButtons.push(getMicrosoftButton);
getButtons.push(getFacebookButton);
getButtons.push(getGitHubButton);
getButtons.push(getAmazonButton);
getButtons.push(getLinkedinButton);

export const SocialLoginButtons = () => {
    const settings = getSocialLoginSettings();
    return (
        <>
            {settings.title != null && <h1 className="title">{settings.title}</h1>}
            {getButtons
                .sort((x1, x2) => x1().position - x2().position)
                .map(value => value().element)}
        </>
    );
}