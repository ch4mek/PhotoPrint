import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { popUpBreakpoints, popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

import { gridAreas, MOBILE_VIEW_REQUEST_FORM_BREAKPOINT } from './constants';

const { participants, details } = gridAreas;

const formBreakpoints = {
	m: 656,
	s: 356,
};

const MIN_POPUP_HEIGHT = 500;

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		outline: `1px solid ${theme.palette['graphics-input-stroke']}`,
		width: popUpSizes.l,
		border: 'none',

		[theme.breakpoints.down(popUpBreakpoints.desktopXS)]: {
			minWidth: formBreakpoints.m,
		},

		[theme.breakpoints.down(MOBILE_VIEW_REQUEST_FORM_BREAKPOINT)]: {
			minWidth: formBreakpoints.s,
		},
	},
	content: {
		minHeight: MIN_POPUP_HEIGHT,
		...getAdaptivePopUpContentStylesConfig({
			hasContols: false,
			theme,
			hasTabs: false,
			windowHeight,
		}),
		paddingRight: theme.spaces.l - 1,
	},
});

export const FormWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));

export const RequestContent = styled('div')(({ theme }) => ({
	display: 'grid',
	gap: theme.spaces.m,
	gridTemplateColumns: 'minmax(0, 1fr) 286px',
	gridTemplateAreas: `
    '${details} ${participants}'
  `,

	[theme.breakpoints.down(popUpBreakpoints.desktopXS)]: {
		gridTemplateColumns: `1fr`,
		gridTemplateAreas: `
    '${participants}'
    '${details}' 
  `,
	},
}));

export const SectionWrapper = styled('div')<{ gridArea: string }>(
	({ theme, gridArea }) => ({
		outline: `1px solid ${theme.palette['graphics-divider-1']}`,
		padding: theme.spaces.m,
		borderRadius: BorderRadiuses.m,
		gridArea,
		height: 'fit-content',
		minWidth: 0,
	})
);
