import { Tabs } from '@mms/mms-ui-library/components/Tabs';

import { useTeamBoardTabs } from '../hooks/useTeamBoardTabs';

import { TabsContainer, TabsContentWrapper } from './styles';
import { TeamBoardUserTabsProps } from './types';

export function TeamBoardUserTabs({
	activeTab,
	onChange,
	accessLevel,
	profileKind,
	children,
}: TeamBoardUserTabsProps) {
	const tabs = useTeamBoardTabs(accessLevel, profileKind);

	return (
		<TabsContainer>
			<Tabs activeTab={activeTab} onChangeTab={onChange} data={tabs} />
			<TabsContentWrapper>{children}</TabsContentWrapper>
		</TabsContainer>
	);
}
