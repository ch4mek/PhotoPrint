import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { breakpoints } from '@mms/mms-ui-library/theme';

const { tablet } = breakpoints;
const TRACK_CELL_MARGIN_TABLET = 73;

export const TimeInputCellWrapper = styled('div')(
	({ theme }) => css`
		${theme.breakpoints.down(tablet)} {
			display: flex;
			height: 100%;
			display: flex;
			align-items: center;

			& > div {
				margin-bottom: ${TRACK_CELL_MARGIN_TABLET}px;
			}
		}
	`
);
