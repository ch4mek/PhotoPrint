import styled from '@emotion/styled';

import { DataGridHoverRowWrapper } from '@/components/Grid/components/DataGridHoverRowWrapper/styles';
import { SMALL_ICON_SIZE } from '@/constants/commonStyles';

import { TEAMBOARD_COLUMNS } from './constants';

export const TeamboardDataGridHoverRowWrapper = styled(DataGridHoverRowWrapper)(
	{
		[`& .${TEAMBOARD_COLUMNS.primarySpecialization.name} button,& .${TEAMBOARD_COLUMNS.primarySpecialization.name} button>svg`]:
			{
				height: `${SMALL_ICON_SIZE}px`,
				width: `${SMALL_ICON_SIZE}px`,
			},
	}
);
