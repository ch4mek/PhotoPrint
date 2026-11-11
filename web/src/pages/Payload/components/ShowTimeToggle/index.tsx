import { useTheme } from '@emotion/react';
import { Switch } from '@mms/mms-ui-library/components/Switch';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { usePayloadContext } from '@/context/PayloadContext';

import { SHOW_TIME } from './constants';
import { SwitchWrapper } from './styles';

export function ShowTimeToggle() {
	const theme = useTheme();
	const { showTime, toggleShowTime } = usePayloadContext();

	return (
		<SwitchWrapper>
			<Typography variant="m-400" color={theme.palette['text-black']}>
				{SHOW_TIME}
			</Typography>
			<Switch checked={showTime} onChange={toggleShowTime} />
		</SwitchWrapper>
	);
}
