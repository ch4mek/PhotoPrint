import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

const FIELDS_WRAPPER_WIDTH = '100%';

export const FieldsWrapper = styled('div')(({ theme }) => ({
	width: FIELDS_WRAPPER_WIDTH,
	display: 'flex',
	flexDirection: 'column',
	rowGap: theme.spaces.l,
}));

export const DateTimeWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: '1fr 1fr',
	gap: theme.spaces.s,
}));

export const getPopUpStylesConfig = (
	theme: Theme,
	windowWidth: number,
	windowHeight: number
) => ({
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowWidth,
		windowHeight,
		hasContols: true,
	}),
});
