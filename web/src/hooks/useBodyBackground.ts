import { useTheme } from '@emotion/react';
import { useLayoutEffect } from 'react';

export const useBodyBackgroundColor = () => {
	const theme = useTheme();

	useLayoutEffect(() => {
		document.body.setAttribute(
			'style',
			`background-color: ${theme.palette['background-white']}`
		);
	}, [theme]);
};
