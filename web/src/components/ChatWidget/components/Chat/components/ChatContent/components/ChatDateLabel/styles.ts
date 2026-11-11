import styled from '@emotion/styled';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import { DEFAULT_DISABLE_OPACITY } from '@/constants/opacities';
import { addOpacityToColor } from '@/helpers/addOpacityToColor';

const CHART_DATE_RADIUS = 16;

export const ChatDateLabelWrapper = styled('li')(() => ({
	display: 'flex',
	justifyContent: 'center',
}));

export const ChatDate = styled('time')<{ isTheFirstDate: boolean }>(
	({ theme, isTheFirstDate }) => ({
		display: 'flex',
		justifyContent: 'center',

		borderRadius: CHART_DATE_RADIUS,
		padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
		marginTop: isTheFirstDate ? theme.spaces.m : theme.spaces.s,

		backgroundColor:
			theme.mode === ThemeMode.LIGHT
				? theme.palette['background-base-1']
				: addOpacityToColor(
						theme.palette['background-base-3'],
						DEFAULT_DISABLE_OPACITY
				  ),
	})
);
