import { Skeleton } from '@mms/mms-ui-library/components/Skeleton';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useState } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { CERTIFICATE_IMAGE_SIZE } from '../../constants';
import { UserCertificatesRow } from '../../types';

import { CertificateImage, SpecializationCellWrapper } from './styles';

export function SpecializationCell({
	row: { certificateImageURL, technology: specializationName },
}: RenderCellProps<UserCertificatesRow>) {
	const [showSkeleton, setShowSkeleton] = useState(true);

	const handleImageLoad = () => {
		setShowSkeleton(false);
	};

	return (
		<SpecializationCellWrapper>
			<Skeleton
				visible={showSkeleton}
				height={CERTIFICATE_IMAGE_SIZE}
				width={CERTIFICATE_IMAGE_SIZE}
			>
				<CertificateImage
					src={certificateImageURL}
					onLoad={handleImageLoad}
					alt={`${specializationName} certificate`}
				/>
			</Skeleton>
			<Typography variant="m-400">{specializationName}</Typography>
		</SpecializationCellWrapper>
	);
}
