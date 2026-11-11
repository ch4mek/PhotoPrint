import { BoardIcon } from '@mms/mms-ui-library/icons/base/BoardIcon';
import { CameraIcon } from '@mms/mms-ui-library/icons/base/CameraIcon';
import { LaptopIcon } from '@mms/mms-ui-library/icons/base/LaptopIcon';
import { ProfileIcon } from '@mms/mms-ui-library/icons/base/ProfileIcon';
import { ScreenIcon } from '@mms/mms-ui-library/icons/base/ScreenIcon';
import { SpeakerphoneIcon } from '@mms/mms-ui-library/icons/base/SpeakerphoneIcon';
import { UsbhubIcon } from '@mms/mms-ui-library/icons/base/UsbhubIcon';

import { MeetingRoomEquipmentTypes } from '../../types';

export const ROOM_NUMBER_SIGN = '№';

export const MEETING_ROOM_EQUIPMENT_LABEL_KEYS = {
	[MeetingRoomEquipmentTypes.SEATS]: 'equipment.seats',
	[MeetingRoomEquipmentTypes.BOARD]: 'equipment.board',
	[MeetingRoomEquipmentTypes.CAMERA]: 'equipment.camera',
	[MeetingRoomEquipmentTypes.LAPTOP]: 'equipment.laptop',
	[MeetingRoomEquipmentTypes.SCREEN]: 'equipment.screen',
	[MeetingRoomEquipmentTypes.SPEAKERPHONE]: 'equipment.speakerphone',
	[MeetingRoomEquipmentTypes.USBHUB]: 'equipment.usbHub',
} as const;

export const MeetingRoomEquipment = {
	[MeetingRoomEquipmentTypes.SEATS]: {
		icon: ProfileIcon,
	},
	[MeetingRoomEquipmentTypes.BOARD]: {
		icon: BoardIcon,
	},
	[MeetingRoomEquipmentTypes.CAMERA]: {
		icon: CameraIcon,
	},
	[MeetingRoomEquipmentTypes.LAPTOP]: {
		icon: LaptopIcon,
	},
	[MeetingRoomEquipmentTypes.SCREEN]: {
		icon: ScreenIcon,
	},
	[MeetingRoomEquipmentTypes.SPEAKERPHONE]: {
		icon: SpeakerphoneIcon,
	},
	[MeetingRoomEquipmentTypes.USBHUB]: {
		icon: UsbhubIcon,
	},
};

export const MIN_AMOUT_EQUIPMENT = 1;
export const MIN_SECTION_HEIGHT = 0;
