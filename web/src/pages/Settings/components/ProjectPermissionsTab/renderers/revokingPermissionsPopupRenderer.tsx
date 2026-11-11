import { Typography } from '@mms/mms-ui-library/components/Typography';

import {
	REVOKING_PERMISSION_TITLE_END,
	REVOKING_PERMISSION_TITLE_START,
} from '../constants';
import { popupJSXContentFactory } from '../factories/popupJSXContentFactory';

export const revokingPermissionsPopupRenderer = (userFullName: string) =>
	popupJSXContentFactory({
		contentBeforeChildren: (
			<>
				{REVOKING_PERMISSION_TITLE_START}
				<Typography variant="m-600">{userFullName} - </Typography>
			</>
		),
		contentAfterChildren: REVOKING_PERMISSION_TITLE_END,
	});
