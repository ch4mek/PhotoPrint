import styled from '@emotion/styled';

import { DEFAULT_OPACITY, HALF_OPACITY } from '@/constants/opacities';

const INPUT_WIDTH = 56;

export const HoursInputWrapper = styled('div')<{
	disabled?: boolean;
	hasError?: boolean;
}>(
	({ theme, disabled, hasError }) =>
		({
			width: INPUT_WIDTH,
			display: 'flex',

			input: {
				height: theme.spacing(5),
				borderRadius: theme.spacing(0.75),
				border: `1px solid ${theme.palette['graphics-input-stroke']}`,
				color: theme.palette['text-black'],
				textAlign: 'center',
				opacity: disabled ? HALF_OPACITY : DEFAULT_OPACITY,
				width: INPUT_WIDTH,
				padding: theme.spacing(0),
				background: theme.palette['background-white'],
				borderColor: hasError ? theme.palette['graphics-error'] : undefined,
				'&:hover': !disabled && {
					textDecoration: 'none',
					border: `1px solid ${theme.palette['graphics-primary-2']}`,
				},
				'&:focus': !disabled && {
					outline: 'none',
					border: `1px solid ${theme.palette['graphics-accent']}`,
				},
			},
		} as const)
);
