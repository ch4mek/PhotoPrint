import { InternSummaryTab } from '../components/InternSummaryTab';
import { TeamBoardUserSummaryTab } from '../components/TeamBoardUserSummaryTab';
import { TeamboardUserProfileKind } from '../types';
import { isInternProfile } from '../utils';

export const getSummaryTabComponent = (profileKind: TeamboardUserProfileKind) =>
	isInternProfile(profileKind) ? InternSummaryTab : TeamBoardUserSummaryTab;
