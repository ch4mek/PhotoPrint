import styled from '@emotion/styled';

import { DataGridHoverRowWrapper } from '@/components/Grid/components/DataGridHoverRowWrapper/styles';
import { SMALL_ICON_SIZE } from '@/constants/commonStyles';

import { ACCESS_USERS_CELL_CLASS } from './constants';

export const ProjectsDataGridHoverRowWrapper = styled(DataGridHoverRowWrapper)({
	minHeight: 0,

	[`& .${ACCESS_USERS_CELL_CLASS} button, & .${ACCESS_USERS_CELL_CLASS} button > svg`]:
		{
			height: `${SMALL_ICON_SIZE}px`,
			width: `${SMALL_ICON_SIZE}px`,
		},
});
