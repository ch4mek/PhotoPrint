import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { breakpoints } from '@mms/mms-ui-library/theme';

const { tablet } = breakpoints;

const DOUBLE_CELL_WIDTH = 56;
const DOUBLE_CELL_MARGIN_TABLET = 70;

export const DoubleCellContentWrapper = styled('div')(
	({ theme }) => css`
		width: ${DOUBLE_CELL_WIDTH}px;
		position: relative;
		display: flex;
		flex-direction: column;

		${theme.breakpoints.down(tablet)} {
			margin-bottom: ${DOUBLE_CELL_MARGIN_TABLET}px;
		}
	`
);
