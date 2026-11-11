import styled from '@emotion/styled';
import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';
import { DEFAULT_GRID_PAGINATION_HEIGHT } from '@mms/mms-ui-library/constants/grid';

import { EquipmentTabs } from './constants';

export const EquipmentPageWrapper = styled('div')`
	height: 100%;
	display: flex;
	flex-direction: column;
`;

export const EquipmentTabsWrapper = styled(Scrollbar)<{
	activeTab: EquipmentTabs;
}>(({ theme, activeTab }) => ({
	padding:
		activeTab === EquipmentTabs.EQUIPMENT
			? `0px ${theme.spaces.m}px ${theme.spaces.m}px ${theme.spaces.m}px`
			: theme.spaces.m,
	display: 'flex',
	flexDirection: 'column',
	height: `calc(100% - ${DEFAULT_GRID_PAGINATION_HEIGHT}px)`,
	flex: '1 1 auto',
}));
