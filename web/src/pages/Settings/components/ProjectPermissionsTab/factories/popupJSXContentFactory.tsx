import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ReactNode } from 'react';

interface PopupJSXContentFactoryProps {
	contentBeforeChildren?: string | ReactNode;
	contentAfterChildren?: string | ReactNode;
}

export function popupJSXContentFactory({
	contentBeforeChildren: textBeforeChildren,
	contentAfterChildren: textAfterChildren,
}: PopupJSXContentFactoryProps) {
	return function PopupJSXContent({ children }: { children: ReactNode }) {
		return (
			<Typography variant="m-400">
				{textBeforeChildren} <Typography variant="m-600">{children}</Typography>
				{textAfterChildren}
			</Typography>
		);
	};
}
