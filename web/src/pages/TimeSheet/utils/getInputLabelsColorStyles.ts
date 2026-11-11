import { css } from '@emotion/react';

export const getInputColorsStyles = (color: string, labelStyle?: string) => css`
	.task-info-container > div > div {
		background-color: ${color};
		border-radius: 6px;
	}

	label {
		background-color: ${color};
		${labelStyle}
	}

	${'.engineer-task-info-cell'} div:not(button) > span {
		background-color: ${color} !important;
		${labelStyle}
	}
`;
