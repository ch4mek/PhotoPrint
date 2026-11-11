import { LinkIcon } from '@mms/mms-ui-library/icons/editor/LinkIcon';
import { FacebookIcon } from '@mms/mms-ui-library/icons/socialMedia/FacebookIcon';
import { InstagramIcon } from '@mms/mms-ui-library/icons/socialMedia/InstagramIcon';
import { LinkedinIcon } from '@mms/mms-ui-library/icons/socialMedia/LinkedinIcon';
import { TelegramIcon } from '@mms/mms-ui-library/icons/socialMedia/TelegramIcon';
import { TiktokIcon } from '@mms/mms-ui-library/icons/socialMedia/TiktokIcon';
import { VkIcon } from '@mms/mms-ui-library/icons/socialMedia/VkIcon';
import { XIcon } from '@mms/mms-ui-library/icons/socialMedia/XIcon';
import { YoutubeIcon } from '@mms/mms-ui-library/icons/socialMedia/YoutubeIcon';

export const getSocialMediaIcon = (link: string) => {
	if (/vk\.com/.test(link)) {
		return <VkIcon />;
	}

	if (/instagram\.com/.test(link)) {
		return <InstagramIcon />;
	}

	if (/x\.com/.test(link)) {
		return <XIcon />;
	}

	if (/tiktok\.com/.test(link)) {
		return <TiktokIcon />;
	}

	if (/linkedin\.com/.test(link)) {
		return <LinkedinIcon />;
	}

	if (/facebook\.com/.test(link)) {
		return <FacebookIcon />;
	}

	if (/youtube\.com/.test(link)) {
		return <YoutubeIcon />;
	}

	if (/t\.me/.test(link)) {
		return <TelegramIcon />;
	}

	return <LinkIcon />;
};
