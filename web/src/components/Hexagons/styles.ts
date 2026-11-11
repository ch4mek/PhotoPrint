import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const LeftHexagonWrapper = styled('div')(
	() => css`
		position: absolute;
		left: 0;
		top: 0;
		display: flex;
	`
);

export const RightHexagonWrapper = styled('div')(
	() => css`
		position: absolute;
		right: 0;
		bottom: 0;
		display: flex;
	`
);
