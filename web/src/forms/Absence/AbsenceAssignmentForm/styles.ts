import { Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Form } from 'formik';

import { popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

import {
	ASSIGNMENT_FORM_MOBILE_BREAKPOINT,
	ASSIGNMENT_FORM_TABLET_BREAKPOINT,
} from './constants';

export const AbsenceFormSectionWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: 'repeat(3, 1fr)',
	rowGap: theme.spaces.l,
	columnGap: theme.spaces.s,

	[theme.breakpoints.down(ASSIGNMENT_FORM_TABLET_BREAKPOINT)]: {
		gridTemplateColumns: `1fr 1fr`,
	},

	[theme.breakpoints.down(ASSIGNMENT_FORM_MOBILE_BREAKPOINT)]: {
		gridTemplateColumns: `1fr`,
	},
}));

export const getPopUpStylesConfig = (
	theme: Theme,
	windowWidth: number,
	windowHeight: number
) => ({
	container: {
		width: popUpSizes.m,
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowWidth,
		windowHeight,
		hasContols: true,
	}),
});

export const FormContent = styled(Form)(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));
