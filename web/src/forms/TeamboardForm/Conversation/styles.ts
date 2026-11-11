import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { scrollbarWidths } from '@/constants/commonStyles';
import { popUpBreakpoints } from '@/constants/popUp';
import { FormWrapper } from '@/forms/styles';

const DATE_PICKERS_WIDTH = 171;

export const DatePickersWrapper = styled('div')(
	({ theme }) => css`
		display: grid;
		column-gap: ${theme.spaces.s}px;
		row-gap: ${theme.spaces.l}px;
		grid-template-columns: repeat(auto-fill, ${DATE_PICKERS_WIDTH}px);

		${theme.breakpoints.down(popUpBreakpoints.mobileL + scrollbarWidths.s)} {
			grid-template-columns: repeat(2, 1fr);
		}
	`
);

export const ConversationFormWrapper = styled(FormWrapper)({
	display: 'grid',
	gridTemplateColumns: 'minmax(0, 1fr)',
	gridTemplateRows: 'repeat(1, minmax(0, 1fr))',
});
