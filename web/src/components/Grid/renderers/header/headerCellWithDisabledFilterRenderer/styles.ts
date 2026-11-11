import styled from '@emotion/styled';

import {
	DEFAULT_DISABLE_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';

export const FilterHeaderCellContentWrapper = styled('div')<{
	disabled: boolean;
}>(({ theme, disabled }) => ({
	display: 'flex',
	justifyContent: 'center',
	gap: theme.spaces.s,
	position: 'relative',
	alignItems: 'center',
	opacity: disabled ? DEFAULT_DISABLE_OPACITY : DEFAULT_OPACITY,

	'&:hover': {
		cursor: disabled ? 'default' : 'pointer',
	},

	'& button': {
		opacity: DEFAULT_OPACITY,
	},
}));
