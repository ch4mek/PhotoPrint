import { Button } from '@mms/mms-ui-library/components/Button';
import { PayloadIcon } from '@mms/mms-ui-library/icons/base/PayloadIcon';
import { useNavigate } from 'react-router-dom';

import { Routes } from '@/constants/routes';
import { CALENDAR_TAB_ID } from '@/constants/settings';
import {
	SETTINGS_TAB_INDEX_STORAGE_KEY,
	SETTINGS_YEAR_FILTER_STORAGE_KEY,
} from '@/constants/storageKeys';
import { useAuth } from '@/context/AuthContext';
import { setItemToStorage } from '@/helpers/storage';
import {
	GO_TO_CALENDAR_BUTTON_TITLE,
	NO_PAYLOAD_DATA_TITLE,
} from '@/pages/Payload/components/PayloadGrid/components/NoPayloadData/constants';
import {
	NoPayloadDataContent,
	NoPayloadDataTitle,
	NoPayloadDataWrapper,
} from '@/pages/Payload/components/PayloadGrid/components/NoPayloadData/styles';

export function NoPayloadData({ year }: { year: number }) {
	const navigate = useNavigate();
	const { id } = useAuth();

	const handleGoToCalendar = () => {
		setItemToStorage(
			`${SETTINGS_TAB_INDEX_STORAGE_KEY}/${id}`,
			CALENDAR_TAB_ID
		);
		setItemToStorage(`${SETTINGS_YEAR_FILTER_STORAGE_KEY}/${id}`, year);
		navigate(Routes.settings);
	};

	return (
		<NoPayloadDataWrapper>
			<NoPayloadDataContent>
				<PayloadIcon />
				<NoPayloadDataTitle variant="m-400">
					{NO_PAYLOAD_DATA_TITLE}
				</NoPayloadDataTitle>
				<Button isFullWidth={false} onClick={handleGoToCalendar}>
					{GO_TO_CALENDAR_BUTTON_TITLE}
				</Button>
			</NoPayloadDataContent>
		</NoPayloadDataWrapper>
	);
}
