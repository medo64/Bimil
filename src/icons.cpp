#include <QImage>
#include "icons.h"

QIcon Icons::app() {
    QIcon icon;
#ifndef QT_DEBUG
    icon.addFile(":icons/16x16/app.png", QSize(16, 16));
    icon.addFile(":icons/24x24/app.png", QSize(24, 24));
    icon.addFile(":icons/32x32/app.png", QSize(32, 32));
    icon.addFile(":icons/48x48/app.png", QSize(48, 48));
    icon.addFile(":icons/64x64/app.png", QSize(64, 64));
    icon.addFile(":icons/96x96/app.png", QSize(96, 96));
    icon.addFile(":icons/128x128/app.png", QSize(128, 128));
#else
    icon.addFile(":icons/16x16/appDebug.png", QSize(16, 16));
    icon.addFile(":icons/24x24/appDebug.png", QSize(24, 24));
    icon.addFile(":icons/32x32/appDebug.png", QSize(32, 32));
    icon.addFile(":icons/48x48/appDebug.png", QSize(48, 48));
    icon.addFile(":icons/64x64/appDebug.png", QSize(64, 64));
    icon.addFile(":icons/96x96/appDebug.png", QSize(96, 96));
    icon.addFile(":icons/128x128/appDebug.png", QSize(128, 128));
#endif
    return icon;
}
