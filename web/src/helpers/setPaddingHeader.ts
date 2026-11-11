import { Theme } from '@emotion/react';

import { Routes } from '@/constants/routes';

export const setPaddingHeader = (name: string, theme: Theme) => {
	if (name === Routes.signIn) {
		return 0;
	}

	return theme.spacing(8);
};
