import { useTheme } from '@emotion/react';
import { Tabs } from '@mms/mms-ui-library/components/Tabs';
import { useCallback, useState } from 'react';

import {
	EQUIPMENT_TABS,
	EquipmentTabs,
	EquipmentTabsTitles,
} from '../../constants';
import { EquipmentPageWrapper, EquipmentTabsWrapper } from '../../styles';

import { TabPanel } from '@/components/TabPanel';

import 'react-data-grid/lib/styles.css';

export function EquipmentPageContent() {
	const theme = useTheme();
	const [selectedTab, setSelectedTab] = useState(EquipmentTabs.EQUIPMENT);

	const handleChangeTab = useCallback(
		(newSelectedTab: number) => setSelectedTab(newSelectedTab),
		[]
	);

	return (
		<EquipmentPageWrapper>
			<Tabs
				activeTab={selectedTab}
				onChangeTab={handleChangeTab}
				data={EquipmentTabsTitles}
				customActiveTabColor={theme.palette['background-base-1']}
			/>
			<EquipmentTabsWrapper
				activeTab={selectedTab}
				scroll="vertical"
				maxWidth="100%"
				maxHeight="100%"
			>
				{EQUIPMENT_TABS.map(({ id: tabId, Component }) => (
					<TabPanel key={tabId} index={tabId} value={selectedTab}>
						<Component />
					</TabPanel>
				))}
			</EquipmentTabsWrapper>
		</EquipmentPageWrapper>
	);
}
