import styled from '@emotion/styled';

export const RenderWrapper = styled('span')<{
	color?: string;
	backgroundColor?: string;
	defaultColor?: string;
}>(({ color, backgroundColor, defaultColor }) => ({
	...(color && {
		color: `${color} !important`,
		'& *:not(span)': {
			color: `${color} !important`,
		},
	}),
	...(backgroundColor && {
		backgroundColor: `${backgroundColor} !important`,
		'& *:not(span)': {
			backgroundColor: `${backgroundColor} !important`,
		},
	}),

	caretColor: defaultColor,
}));
