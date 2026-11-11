import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import { EmptyResultWrapper } from './styles';
import { EmptyResultProps } from './types';

export function EmptyResult({
	darkThemeIcon,
	lightThemeIcon,
	title,
}: EmptyResultProps) {
	const theme = useTheme();

	return (
		<EmptyResultWrapper>
			{theme.mode === ThemeMode.DARK ? darkThemeIcon : lightThemeIcon}
			<Typography variant="m-400" color={theme.palette['text-black']}>
				{title}
			</Typography>
		</EmptyResultWrapper>
	);
}
