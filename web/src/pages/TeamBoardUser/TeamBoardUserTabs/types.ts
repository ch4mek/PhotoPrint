import { PropsWithChildren } from 'react';

import { TeamboardUserAccessLevel, TeamboardUserProfileKind } from '../types';

export interface TeamBoardUserTabsProps extends PropsWithChildren {
	activeTab: number;
	onChange: (newTab: number) => void;
	accessLevel: TeamboardUserAccessLevel;
	profileKind: TeamboardUserProfileKind;
}
