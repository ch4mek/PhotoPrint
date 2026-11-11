import { Tabs } from '@mms/mms-ui-library/components/Tabs';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { MEETING_ROOMS_TAB_ID } from '@/constants/booking';

import { SpacesTabsProps } from './types';

export function SpacesTabs({
	activeTab,
	onChange,
	spaces,
	hasMeetingRooms,
	activeTabsId,
}: SpacesTabsProps) {
	const { t } = useTranslation('booking');

	const tabs = useMemo(() => {
		const spacesTabs = spaces.map(({ name, internalCode, id }) => ({
			id,
			text: name,
			disabled: !activeTabsId?.includes(internalCode),
		}));

		if (hasMeetingRooms) {
			spacesTabs.push({
				id: MEETING_ROOMS_TAB_ID,
				text: t('spaces.meetingRooms'),
				disabled: !activeTabsId?.includes(MEETING_ROOMS_TAB_ID),
			});
		}

		return spacesTabs;
	}, [spaces, hasMeetingRooms, activeTabsId, t]);

	return <Tabs activeTab={activeTab} onChangeTab={onChange} data={tabs} />;
}
