import styled from '@emotion/styled';

import { getGridTemplateColumnsForCollapsibleTable } from '../../utils';

import { checkIfForwardProps } from '@/helpers/checkIfForwardProps';

const PROPS_TO_CHECK = ['isDisplayFact'];

export const TaskTableRowItemContainer = styled('div', {
	shouldForwardProp: checkIfForwardProps(PROPS_TO_CHECK),
})<{ isDisplayFact?: boolean }>(({ isDisplayFact }) => ({
	display: 'grid',
	gridTemplateColumns: getGridTemplateColumnsForCollapsibleTable({
		isDisplayFact,
		isTaskRow: true,
	}),
	width: '100%',
}));

export const DateTitleContainer = styled.div`
	display: flex;
	flex-direction: column;
`;

export const DurationWrapper = styled.div`
	display: flex;
	justify-content: left;
`;

export const DurationContainer = styled.div`
	display: flex;
	flex-direction: column;
`;

export const TaskDescriptionContainer = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	paddingRight: `${theme.spaces.xl2}px`,
}));
