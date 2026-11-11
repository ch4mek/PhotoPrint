import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const TabTableWrapper = styled('div')(
	() => css`
		flex-grow: 1;
		min-height: 0;
		display: flex;
		flex-direction: column;
	`
);
