import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const CellWrapper = styled.div(
	() => css`
		height: 100%;
		width: 100%;
		text-overflow: ellipsis;
		overflow: hidden;
	`
);
