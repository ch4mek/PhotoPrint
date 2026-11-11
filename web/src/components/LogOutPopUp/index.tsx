import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useEffect, useState } from 'react';

import { timeUnits } from '@/constants/date';
import { zIndexes } from '@/constants/zIndexes';

import {
	LOG_OUT_MESSAGE,
	LOG_OUT_MESSAGE_TIME,
	NEGATIVE_CONTROL_TEXT,
	POPUP_TITLE,
	POSITIVE_CONTROL_TEXT,
} from './constants';
import { LogOutPopUpProps } from './types';
import {
	calculateSecondsToLogOut,
	checkTimeActivity,
	getFormattedSeconds,
} from './utils';

export function LogOutPopUp({
	handleLogOut,
	handleStayLogIn,
}: LogOutPopUpProps) {
	const [secondsToLogOut, setSecondsToLogOut] = useState(
		calculateSecondsToLogOut
	);

	useEffect(() => {
		const id = setInterval(() => {
			setSecondsToLogOut((prev) => prev - 1);
		}, timeUnits.MILLISECONDS_IN_SECOND);

		return () => {
			clearInterval(id);
		};
	}, []);

	if (checkTimeActivity()) {
		handleLogOut();

		return null;
	}

	return (
		<PopUp
			title={POPUP_TITLE}
			onClose={handleStayLogIn}
			type="approve"
			showCloseButton={false}
			controls={{
				positiveControl: {
					onClick: handleStayLogIn,
					text: POSITIVE_CONTROL_TEXT,
				},
				negativeControl: {
					onClick: handleLogOut,
					text: NEGATIVE_CONTROL_TEXT,
				},
			}}
			zIndex={zIndexes.logOutPopUp}
		>
			<Typography variant="m-400">
				{LOG_OUT_MESSAGE}
				<Typography variant="m-600">
					{getFormattedSeconds(secondsToLogOut)}
				</Typography>
				{LOG_OUT_MESSAGE_TIME}
			</Typography>
		</PopUp>
	);
}
