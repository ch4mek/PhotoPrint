import styled from '@emotion/styled';

import { ChevronButtonWrapperProps } from './types';

export const ChevronButtonWrapper = styled('span')<ChevronButtonWrapperProps>(
	({ isActive, theme, disabled, chevronColor, activeColor }) => {
		const defaultColor =
			isActive && activeColor
				? activeColor
				: theme.palette['graphics-light-gray'];

		const finalColor = chevronColor || defaultColor;

		return {
			display: 'inline-flex',
			alignItems: 'center',
			transformOrigin: 'center',
			transition: 'transform 0.3s ease, color 0.3s ease, opacity 0.3s ease',
			transform: isActive ? 'rotate(180deg)' : 'rotate(0deg)',

			color: finalColor,

			...(!disabled && {
				'&:hover': {
					color: finalColor,
					cursor: 'pointer',
				},
			}),
		};
	}
);
