import styled from '@emotion/styled';
import { DEFAULT_GRID_PAGINATION_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { lightTheme } from '@mms/mms-ui-library/theme';

import { SettingsPageGridWrapper } from '../../styles';

import { getDataGridResetStyles } from '@/components/Grid/styles';

const BOTTOM_INDENTATION =
	DEFAULT_GRID_PAGINATION_HEIGHT - lightTheme.spaces.m + 1;

export const BillableWorkingHoursPermissionsGridWrapper = styled(
	SettingsPageGridWrapper
)({
	height: `calc(100% - ${BOTTOM_INDENTATION}px)`,
	...getDataGridResetStyles(),
});
