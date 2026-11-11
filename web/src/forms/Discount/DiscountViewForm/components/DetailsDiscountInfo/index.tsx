import { useTheme } from '@emotion/react';
import { CallIcon } from '@mms/mms-ui-library/icons/base/CallIcon';
import { GlobalIcon } from '@mms/mms-ui-library/icons/base/GlobalIcon';
import { MapPinIcon } from '@mms/mms-ui-library/icons/base/MapPinIcon';
import { useMemo } from 'react';

import { ViewImage } from '@/components/ViewImage';
import {
	AppLinkWrapper,
	CompanyTypeText,
	ContactInfoItem,
	ContactsInfo,
	DetailInfoWrapper,
	DetailsDiscountInfoWrapper,
	DetailsInfoText,
} from '@/forms/Discount/DiscountViewForm/components/DetailsDiscountInfo/styles';
import { DetailsDiscountInfoProps } from '@/forms/Discount/DiscountViewForm/components/DetailsDiscountInfo/types';
import { SocialMediaLinks } from '@/forms/Discount/DiscountViewForm/components/SocialMediaLinks';
import { getDomenFromUrl } from '@/forms/Discount/DiscountViewForm/utils/getDomenFromUrl';

export function DetailsDiscountInfo({
	discountData,
	ref,
}: DetailsDiscountInfoProps) {
	const theme = useTheme();
	const mediaLinks = useMemo(
		() =>
			discountData &&
			discountData?.discountReferences &&
			discountData.discountReferences.map(
				({ socialMediaUrl }) => socialMediaUrl
			),
		[discountData]
	);

	return (
		<DetailsDiscountInfoWrapper ref={ref}>
			<ViewImage
				imageId={discountData?.imageFileId}
				title={discountData?.name || ''}
				type="full"
			/>
			<DetailInfoWrapper>
				{discountData?.companyType && (
					<CompanyTypeText variant="m-400" color={theme.palette['text-gray']}>
						{discountData.companyType}
					</CompanyTypeText>
				)}
				<ContactsInfo>
					{discountData?.website && (
						<ContactInfoItem>
							<GlobalIcon />
							<AppLinkWrapper target="_blank" href={discountData.website}>
								{getDomenFromUrl(discountData.website)}
							</AppLinkWrapper>
						</ContactInfoItem>
					)}
					{discountData?.phoneNumber && (
						<ContactInfoItem>
							<CallIcon />
							<DetailsInfoText>{discountData.phoneNumber}</DetailsInfoText>
						</ContactInfoItem>
					)}
					{discountData?.location && (
						<ContactInfoItem>
							<MapPinIcon />
							<DetailsInfoText>{discountData.location}</DetailsInfoText>
						</ContactInfoItem>
					)}
				</ContactsInfo>
				<SocialMediaLinks links={mediaLinks || []} />
			</DetailInfoWrapper>
		</DetailsDiscountInfoWrapper>
	);
}
