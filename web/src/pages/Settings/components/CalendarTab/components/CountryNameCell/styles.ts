import styled from '@emotion/styled';
import { Input } from '@mms/mms-ui-library/components/Input';

import { DEFAULT_OPACITY, HALF_OPACITY } from '@/constants/opacities';

export const CountryNameInputWrapper = styled('div')(() => ({
	width: '100%',
}));

export const CountryNameInput = styled(Input)(({ disabled, theme }) => ({
	input: {
		padding: `0 ${theme.spaces.m}px 0`,
		opacity: DEFAULT_OPACITY,
		height: 'auto',
	},

	opacity: disabled ? HALF_OPACITY : DEFAULT_OPACITY,
}));
