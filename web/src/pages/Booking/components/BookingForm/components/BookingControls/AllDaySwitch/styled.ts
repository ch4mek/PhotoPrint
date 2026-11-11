import styled from '@emotion/styled';

import { switchGridAreas } from '../constants';

export const AllDaySwitchWrapper = styled('label')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
	gridArea: switchGridAreas.allDaySwitch,
	color: theme.palette['text-gray'],
}));
