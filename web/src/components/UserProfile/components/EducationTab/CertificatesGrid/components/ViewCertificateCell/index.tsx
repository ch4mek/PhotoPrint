import { Button } from '@mms/mms-ui-library/components/Button';
import { RenderCellProps } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { UserCertificatesRow } from '../../types';

export function ViewCertificateCell({
	row: { certificateViewLink },
}: RenderCellProps<UserCertificatesRow>) {
	const { t } = useTranslation('common', { keyPrefix: 'controlButtons' });
	const handleClick = () => {
		window.open(certificateViewLink);
	};

	return (
		<Button view="light" isFullWidth={false} onClick={handleClick}>
			{t('view')}
		</Button>
	);
}
