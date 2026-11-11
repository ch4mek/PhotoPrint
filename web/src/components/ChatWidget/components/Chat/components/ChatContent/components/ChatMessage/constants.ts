import { Theme } from '@emotion/react';

import { createGradient } from '@/helpers/createGradient';
import { ThemeMode, WHITE } from '@/theme';
import { ChatRole } from '@/types/chatWidget';

const { LIGHT, DARK } = ThemeMode;
const LIGHT_OPACITY = 0.7;

export const messageColorMap = (theme: Theme) => ({
	[ChatRole.User]: {
		[LIGHT]: createGradient(
			theme.palette['graphics-accent'],
			WHITE,
			LIGHT_OPACITY
		),
		[DARK]: theme.palette['graphics-accent'],
	},
	[ChatRole.Assistant]: {
		[LIGHT]: theme.palette['background-base-2'],
		[DARK]: theme.palette['background-base-3'],
	},
});

export const messageTextColorMap = (theme: Theme, mainColor: string) => ({
	[ChatRole.User]: {
		[LIGHT]: mainColor,
		[DARK]: theme.palette['text-white'],
	},
	[ChatRole.Assistant]: {
		[LIGHT]: mainColor,
		[DARK]: mainColor,
	},
});

export const loadingMessageOptions = {
	id: 0,
	role: ChatRole.Assistant,
	message: null,
	createdAtUtc: '',
};
