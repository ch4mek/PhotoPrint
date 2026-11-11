import { Theme } from '@emotion/react';

export interface AllBirthdaysPopupProps {
	closePopup: VoidFunction;
}

export interface GetPopUpStylesConfigProps {
	theme: Theme;
	windowHeight: number;
	windowWidth?: number;
}
