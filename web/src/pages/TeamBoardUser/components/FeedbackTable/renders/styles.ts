import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const LinkCellWrapper = styled('div')(
	() => css`
		word-break: break-word;
	`
);

export const CustomLink = styled('span')(
	() => css`
		text-decoration: underline;
		text-underline-offset: 3px;
		cursor: pointer;
	`
);
