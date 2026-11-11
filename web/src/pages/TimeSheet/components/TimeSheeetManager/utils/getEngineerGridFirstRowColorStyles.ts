import { css } from '@emotion/react';

export const getEngineerGridFirstRowBorderColorStyles = (color: string) => css`
	border-bottom: 1px solid ${color};
	border-top: 1px solid ${color};
`;
